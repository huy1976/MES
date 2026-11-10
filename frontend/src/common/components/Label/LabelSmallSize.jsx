import clsx from 'clsx';
import styles from './Label.module.scss';

const LabelSmallSize = ({ className = "", children, style }) => {
    return <div className={clsx(styles.labelSmallSize, className)} style={style}>{children}</div>;
  };
  
  export default LabelSmallSize;
  