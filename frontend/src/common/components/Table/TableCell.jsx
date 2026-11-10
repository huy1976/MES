import clsx from 'clsx';
import styles from './Table.module.scss';

const TableCell = ({ children, className = "", style}) => {
    return (
      <td className={clsx(styles.tableCell, className)} style={style}>
        {children}
      </td>
    );
  };
  
  export default TableCell;
  